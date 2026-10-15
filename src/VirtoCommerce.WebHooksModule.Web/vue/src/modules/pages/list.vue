<template>
  <VcBlade
    :title="$t('WEBHOOKS.LIST.TITLE')"
    :subtitle="$t('WEBHOOKS.LIST.SUBTITLE')"
    :toolbar-items="toolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="keyword"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      state-key="WEBHOOKS"
      :active-item-id="activeId"
      :show-all-columns="!activeId"
      :row-actions="rowActions"
      row-actions-mode="dropdown"
      row-actions-position="column"
      :empty-state="{ icon: 'lucide-webhook', title: $t('WEBHOOKS.LIST.EMPTY') }"
      @row-click="(e: { data: Row }) => open(e.data.id)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="name"
        :title="$t('WEBHOOKS.FIELDS.NAME')"
        mobile-role="title"
        sortable
        always-visible
      >
        <template #body="{ data }">
          <span class="webhooks-list__name">
            <VcIcon
              v-if="data.badData"
              icon="lucide-triangle-alert"
              size="s"
              class="webhooks-list__warning"
              :title="$t('WEBHOOKS.DETAILS.BAD_DATA')"
            />
            {{ data.name }}
          </span>
        </template>
      </VcColumn>
      <VcColumn
        id="isActive"
        :title="$t('WEBHOOKS.LIST.ACTIVE')"
        mobile-role="status"
        width="140px"
        min-width="120px"
        sortable
        always-visible
      >
        <template #body="{ data }">
          <div class="dash-inline">
            <VcStatus :variant="data.isActive ? 'success' : 'info'">
              {{ $t(data.isActive ? "WEBHOOKS.LIST.YES" : "WEBHOOKS.LIST.NO") }}
            </VcStatus>
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="successPercent"
        :title="$t('WEBHOOKS.LIST.SUCCESS_PERCENT')"
        type="number"
        mobile-role="field"
        width="150px"
      />
      <VcColumn
        id="errorCount"
        :title="$t('WEBHOOKS.LIST.ERRORS')"
        type="number"
        mobile-role="field"
        width="150px"
      />
      <VcColumn
        id="totalCount"
        :title="$t('WEBHOOKS.LIST.TOTAL')"
        type="number"
        mobile-role="field"
        width="140px"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  useAsync,
  useBlade,
  useDataTablePagination,
  useDataTableSort,
  useFunctions,
  useLoading,
  usePermissions,
  usePopup,
  useTableSearch,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcIcon, VcStatus } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { api, isMultiEvent, type Webhook } from "../api";

defineBlade({
  url: "/webhooks",
  name: "WebHooks",
  isWorkspace: true,
  permissions: ["webhooks:access"],
});

interface Row extends Webhook {
  successPercent: number;
  totalCount: number;
  badData: boolean;
}

const PAGE_SIZE = 20;
const { t } = useI18n({ useScope: "global" });
const { openBlade, closeChildren, exposeToChildren } = useBlade();
const { showConfirmation } = usePopup();
const { hasAccess } = usePermissions();
const { debounce } = useFunctions();

const items = ref<Row[]>([]);
const totalCount = ref(0);
const selection = ref<Row[]>([]);
const activeId = ref<string>();
const { searchValue: keyword } = useTableSearch({ stateKey: "WEBHOOKS" });
const { sortField, sortOrder, sortExpression } = useDataTableSort({ stateKey: "WEBHOOKS", initialField: "name", initialDirection: "ASC" });
const pagination = useDataTablePagination({ stateKey: "WEBHOOKS", pageSize: PAGE_SIZE, totalCount });

// the AngularJS list's derived columns
const toRow = (w: Webhook): Row => {
  const total = (w.errorCount ?? 0) + (w.successCount ?? 0);
  return {
    ...w,
    totalCount: total,
    successPercent: total ? Number((((w.successCount ?? 0) * 100) / total).toFixed(2)) : 0,
    badData: isMultiEvent(w),
  };
};

const { loading: searchLoading, action: load } = useAsync(async () => {
  const result = await api.search({
    searchPhrase: keyword.value || undefined,
    sort: sortExpression.value,
    skip: pagination.skip,
    take: PAGE_SIZE,
  });
  items.value = (result.results ?? []).map(toRow);
  totalCount.value = result.totalCount ?? 0;
});

const { loading: deleteLoading, action: removeAction } = useAsync<string[]>(async (ids) => {
  await api.delete(ids!);
});

const loading = useLoading(searchLoading, deleteLoading);

load();
watch(keyword, () => pagination.setPage(1));
watch([keyword, sortExpression, () => pagination.skip], debounce(() => load(), 300));

function open(id?: string) {
  // set once the blade opened: the replaced blade's onClose runs first
  openBlade({
    name: "WebHookDetails",
    param: id,
    onOpen: () => (activeId.value = id),
    onClose: () => (activeId.value = undefined),
  });
}

async function remove(list: Row[]) {
  if (!(await showConfirmation(t("WEBHOOKS.LIST.DELETE_CONFIRM", { count: list.length })))) return;
  await closeChildren();
  await removeAction(list.map((x) => x.id!));
  selection.value = [];
  await load();
}

// the AngularJS row context menu: Manage, Delete
const rowActions = (item: Row) => [
  { id: "manage", icon: "lucide-pencil", title: t("WEBHOOKS.TOOLBAR.MANAGE"), type: "info" as const, clickHandler: () => open(item.id) },
  {
    id: "delete",
    icon: "lucide-trash-2",
    title: t("WEBHOOKS.TOOLBAR.DELETE"),
    type: "danger" as const,
    variant: "danger" as const,
    hidden: !hasAccess("webhooks:delete"),
    clickHandler: () => remove([item]),
  },
];

const toolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("WEBHOOKS.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "add",
    title: t("WEBHOOKS.TOOLBAR.ADD"),
    icon: "lucide-plus",
    permissions: ["webhooks:update"],
    clickHandler: () => open(),
  },
  {
    id: "delete",
    title: t("WEBHOOKS.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: ["webhooks:delete"],
    disabled: computed(() => !selection.value.length),
    clickHandler: () => remove(selection.value),
  },
]);

exposeToChildren({ reload: () => load() });
</script>

<style lang="scss">
.webhooks-list {
  &__name {
    display: inline-flex;
    align-items: center;
    gap: 6px;
  }
  &__warning {
    color: var(--color-warning-500);
  }
}
</style>
