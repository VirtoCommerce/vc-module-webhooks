<template>
  <VcBlade
    :title="$t('WEBHOOKS.ERRORS.TITLE')"
    :subtitle="options?.name ?? $t('WEBHOOKS.ERRORS.SUBTITLE')"
    :toolbar-items="toolbar"
    :loading="loading"
    width="60%"
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
      state-key="WEBHOOK_ERRORS"
      :row-actions="rowActions"
      row-actions-mode="dropdown"
      row-actions-position="column"
      :empty-state="{ icon: 'lucide-circle-check', title: $t('WEBHOOKS.ERRORS.EMPTY') }"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="eventId"
        :title="$t('WEBHOOKS.FIELDS.EVENT')"
        mobile-role="title"
        width="180px"
        sortable
        always-visible
      >
        <template #body="{ data }">{{ names[data.eventId] ?? data.eventId }}</template>
      </VcColumn>
      <VcColumn
        id="error"
        :title="$t('WEBHOOKS.ERRORS.ERROR')"
        mobile-role="field"
        :line-clamp="3"
      >
        <template #body="{ data }">
          <span :title="data.error">{{ data.error }}</span>
        </template>
      </VcColumn>
      <VcColumn
        id="createdDate"
        :title="$t('WEBHOOKS.ERRORS.CREATED')"
        type="datetime"
        width="160px"
        :visible="false"
        sortable
      />
      <VcColumn
        id="attemptCount"
        :title="$t('WEBHOOKS.ERRORS.ATTEMPTS')"
        type="number"
        mobile-role="field"
        width="110px"
        sortable
      />
      <VcColumn
        id="modifiedDate"
        :title="$t('WEBHOOKS.ERRORS.LAST_ATTEMPT')"
        type="datetime"
        mobile-role="field"
        width="160px"
        sortable
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
import { VcBlade, VcColumn, VcDataTable } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { api, ERROR_RECORD, eventNames, type FeedEntry } from "../api";

defineBlade({
  url: "/webhook-errors",
  name: "WebHookErrors",
  permissions: ["webhooks:feed:read"],
});

const PAGE_SIZE = 20;
const { t, locale } = useI18n({ useScope: "global" });
const { param, options, callParent } = useBlade<{ name?: string }>();
const { showConfirmation } = usePopup();
const { hasAccess } = usePermissions();
const { debounce } = useFunctions();

const items = ref<FeedEntry[]>([]);
const totalCount = ref(0);
const selection = ref<FeedEntry[]>([]);
const { searchValue: keyword } = useTableSearch({ stateKey: "WEBHOOK_ERRORS" });
const names = ref<Record<string, string>>({});
const { sortField, sortOrder, sortExpression } = useDataTableSort({
  stateKey: "WEBHOOK_ERRORS",
  initialField: "eventId",
  initialDirection: "ASC",
});
const pagination = useDataTablePagination({ stateKey: "WEBHOOK_ERRORS", pageSize: PAGE_SIZE, totalCount });

const { loading: searchLoading, action: load } = useAsync(async () => {
  const [result, texts] = await Promise.all([
    api.searchFeed({
      webHookIds: [param.value],
      recordTypes: [ERROR_RECORD],
      // the server matches the phrase against the event id
      searchPhrase: keyword.value || undefined,
      sort: sortExpression.value,
      skip: pagination.skip,
      take: PAGE_SIZE,
    }),
    eventNames(),
  ]);
  names.value = texts;
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const { loading: deleteLoading, action: removeAction } = useAsync<string[]>(async (ids) => {
  await api.deleteFeed(ids!);
});

const loading = useLoading(searchLoading, deleteLoading);

load();
watch(keyword, () => pagination.setPage(1));
watch([keyword, sortExpression, () => pagination.skip], debounce(() => load(), 300));
// the event titles follow the user's language switches
watch(locale, async (lang) => {
  const titles = await eventNames(lang);
  if (lang === locale.value) names.value = titles;
});

async function remove(list: FeedEntry[]) {
  if (!(await showConfirmation(t("WEBHOOKS.ERRORS.DELETE_CONFIRM", { count: list.length })))) return;
  await removeAction(list.map((x) => x.id!));
  selection.value = [];
  await load();
  await callParent("reload");
}

// the AngularJS row context menu's Delete
const rowActions = (entry: FeedEntry) => [
  {
    id: "delete",
    icon: "lucide-trash-2",
    title: t("WEBHOOKS.TOOLBAR.DELETE"),
    type: "danger" as const,
    variant: "danger" as const,
    hidden: !hasAccess("webhooks:delete"),
    clickHandler: () => remove([entry]),
  },
];

const toolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("WEBHOOKS.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "delete",
    title: t("WEBHOOKS.TOOLBAR.REMOVE"),
    icon: "lucide-trash-2",
    permissions: ["webhooks:delete"],
    disabled: computed(() => !selection.value.length),
    clickHandler: () => remove(selection.value),
  },
]);
</script>
