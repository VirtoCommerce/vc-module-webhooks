<template>
  <VcBlade
    :title="isNew ? $t('WEBHOOKS.DETAILS.NEW_TITLE') : stored.name"
    :subtitle="$t('WEBHOOKS.DETAILS.SUBTITLE')"
    :toolbar-items="toolbar"
    :loading="loading"
    :modified="form.isModified.value"
    width="50%"
  >
    <VcContainer>
      <VcForm class="webhook-form">
        <VcBanner
          v-if="badData"
          variant="warning"
          icon="lucide-triangle-alert"
        >
          {{ $t("WEBHOOKS.DETAILS.BAD_DATA") }}
        </VcBanner>
        <VcHint>{{ $t("WEBHOOKS.DETAILS.DESCRIPTION") }}</VcHint>

        <VcCard :header="$t('WEBHOOKS.DETAILS.GENERAL')">
          <div class="webhook-card">
            <VcInput
              v-model="item.name"
              :label="$t('WEBHOOKS.FIELDS.NAME')"
              :placeholder="$t('WEBHOOKS.DETAILS.NAME_PLACEHOLDER')"
              :hint="$t('WEBHOOKS.DETAILS.NAME_HINT')"
              :disabled="badData"
              :error-message="item.name ? undefined : $t('WEBHOOKS.DETAILS.REQUIRED')"
              required
            />
            <VcSelect
              :model-value="eventId"
              :label="$t('WEBHOOKS.FIELDS.EVENT')"
              :placeholder="$t('WEBHOOKS.DETAILS.EVENT_PLACEHOLDER')"
              :hint="$t('WEBHOOKS.DETAILS.EVENT_HINT')"
              :options="events"
              :disabled="badData"
              :clearable="false"
              searchable
              @update:model-value="(v) => setEvent(v as string)"
            />
            <div>
              <VcSelect
                :model-value="payloadNames"
                :label="$t('WEBHOOKS.FIELDS.PAYLOADS', { count: payloadNames.length })"
                :hint="$t('WEBHOOKS.DETAILS.PAYLOADS_HINT')"
                :options="propertyOptions"
                :disabled="badData || !discovered"
                multiple
                searchable
                @update:model-value="(v) => setPayloads(v as string[])"
              />
              <VcHint class="webhook-form__note">{{ $t("WEBHOOKS.DETAILS.PAYLOADS_NOTE") }}</VcHint>
            </div>
            <VcSwitch
              v-model="item.isActive"
              :label="$t('WEBHOOKS.FIELDS.IS_ACTIVE')"
              :disabled="badData"
            />
            <VcInput
              v-model="item.url"
              :label="$t('WEBHOOKS.FIELDS.URL')"
              :hint="$t('WEBHOOKS.DETAILS.URL_HINT')"
              placeholder="https://"
              :disabled="badData"
              :error-message="item.url ? undefined : $t('WEBHOOKS.DETAILS.REQUIRED')"
              required
            />
          </div>
        </VcCard>

        <VcCard :header="$t('WEBHOOKS.FIELDS.AUTH_TYPE')">
          <div class="webhook-card">
            <VcSelect
              v-model="item.authType"
              :label="$t('WEBHOOKS.FIELDS.AUTH_TYPE')"
              :options="authOptions"
              :clearable="false"
            />
            <template v-if="item.authType === 'Basic'">
              <VcInput
                v-model="item.basicUsername"
                :label="$t('WEBHOOKS.FIELDS.BASIC_USERNAME')"
                :hint="$t('WEBHOOKS.DETAILS.BASIC_USERNAME_HINT')"
                :disabled="badData"
              />
              <VcInput
                v-model="item.basicPassword"
                type="password"
                :label="$t('WEBHOOKS.FIELDS.BASIC_PASSWORD')"
                :hint="$t('WEBHOOKS.DETAILS.BASIC_PASSWORD_HINT')"
                :disabled="badData"
              />
            </template>
            <VcInput
              v-else-if="item.authType === 'BearerToken'"
              v-model="item.bearerToken"
              :label="$t('WEBHOOKS.FIELDS.BEARER_TOKEN')"
              :hint="$t('WEBHOOKS.DETAILS.BEARER_TOKEN_HINT')"
              :disabled="badData"
            />
            <template v-else-if="item.authType === 'CustomHeader'">
              <VcInput
                v-model="item.customHttpHeaderName"
                :label="$t('WEBHOOKS.FIELDS.HEADER_NAME')"
                :hint="$t('WEBHOOKS.DETAILS.HEADER_NAME_HINT')"
                :disabled="badData"
              />
              <VcInput
                v-model="item.customHttpHeaderValue"
                :label="$t('WEBHOOKS.FIELDS.HEADER_VALUE')"
                :hint="$t('WEBHOOKS.DETAILS.HEADER_VALUE_HINT')"
                :disabled="badData"
              />
            </template>
          </div>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  notification,
  useAsync,
  useBlade,
  useBladeForm,
  useBladeWidgets,
  useLoading,
  usePermissions,
  usePopup,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBanner, VcBlade, VcCard, VcContainer, VcForm, VcHint, VcInput, VcSelect, VcSwitch } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { api, ERROR_RECORD, eventNames, isMultiEvent, registeredEvents, type Webhook } from "../api";

defineBlade({
  url: "/webhook",
  name: "WebHookDetails",
  permissions: ["webhooks:read"],
});

const MAX_PAYLOADS = 10;
const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, closeSelf, replaceWith, exposeToChildren } = useBlade();
const { hasAccess } = usePermissions();
const { showConfirmation } = usePopup();

const isNew = computed(() => !param.value);
const item = ref<Webhook>({});
const stored = ref<Webhook>({});
const events = ref<{ id: string; title: string }[]>([]);
const properties = ref<string[]>([]);
const discovered = ref(false);
const errorCount = ref(0);

const badData = computed(() => isMultiEvent(stored.value));

const eventId = computed(() => item.value.events?.[0]?.eventId);
const payloadNames = computed(() => (item.value.payloads ?? []).map((p) => p.eventPropertyName!));
const propertyOptions = computed(() => properties.value.map((p) => ({ id: p, title: p })));
const authOptions = computed(() =>
  (["None", "Basic", "BearerToken", "CustomHeader"] as const).map((id) => ({ id, title: t(`WEBHOOKS.AUTH_TYPES.${id}`) })),
);

async function loadProperties(id?: string) {
  properties.value = [];
  discovered.value = false;
  if (!id || !events.value.some((e) => e.id === id)) return;
  const result = await api.properties(id);
  properties.value = result.properties ?? [];
  discovered.value = !!result.discovered;
}

function setEvent(id: string) {
  if (id === eventId.value) return;
  // another event has other properties: the chosen payload fields go
  item.value.payloads = [];
  item.value.events = [{ eventId: id, displayName: events.value.find((e) => e.id === id)?.title }];
  loadProperties(id);
}

function setPayloads(names: string[]) {
  item.value.payloads = (names ?? []).slice(0, MAX_PAYLOADS).map((eventPropertyName) => ({ eventPropertyName }));
}

const form = useBladeForm({
  data: item,
  canSaveOverride: computed(() => !!item.value.name && !!item.value.url && !badData.value && hasAccess("webhooks:update")),
  closeConfirmMessage: computed(() => t("WEBHOOKS.DETAILS.CLOSE_CONFIRM")),
});

// the feed (and the errors blade) needs webhooks:feed:read; without it the webhook still opens, with no errors widget
const canReadFeed = computed(() => hasAccess("webhooks:feed:read"));

async function countErrors() {
  if (isNew.value || !canReadFeed.value) return;
  errorCount.value = (await api.searchFeed({ webHookIds: [param.value], recordTypes: [ERROR_RECORD], take: 0 })).totalCount ?? 0;
}

const { loading: loadLoading, action: load } = useAsync(async () => {
  const [all, names, webhook] = await Promise.all([
    registeredEvents(),
    eventNames(),
    isNew.value ? Promise.resolve<Webhook>({ authType: "None", isActive: false, events: [], payloads: [] }) : api.get(param.value!),
  ]);
  events.value = all
    .map((e) => ({ id: e.id, title: names[e.id] ?? e.displayName ?? e.id }))
    .sort((a, b) => a.title.localeCompare(b.title));
  webhook.contentType = "application/json";
  webhook.events = (webhook.events ?? []).map((e) => ({ eventId: e.eventId, displayName: e.displayName }));
  webhook.payloads = (webhook.payloads ?? []).map((p) => ({ eventPropertyName: p.eventPropertyName }));
  webhook.authType ??= "None";
  item.value = webhook;
  stored.value = JSON.parse(JSON.stringify(webhook));
  form.setBaseline();
  await Promise.all([loadProperties(eventId.value), countErrors()]);
});

const { loading: saveLoading, action: save } = useAsync(async () => {
  const [saved] = await api.save([item.value]);
  notification.success(t("WEBHOOKS.DETAILS.SAVED"));
  await callParent("reload");
  if (isNew.value) {
    form.setBaseline();
    await replaceWith({ name: "WebHookDetails", param: saved.id });
    return;
  }
  await load();
});

const loading = useLoading(loadLoading, saveLoading);

useBladeWidgets([
  {
    id: "WebHookErrors",
    icon: "lucide-circle-alert",
    title: "WEBHOOKS.WIDGETS.ERRORS",
    badge: errorCount,
    isVisible: computed(() => !isNew.value && canReadFeed.value),
    onClick: () => openBlade({ name: "WebHookErrors", param: param.value, options: { name: stored.value.name } }),
    onRefresh: countErrors,
  },
]);

// isNew is fixed for the blade: a created webhook reopens through replaceWith
const toolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: isNew.value ? t("WEBHOOKS.TOOLBAR.CREATE") : t("WEBHOOKS.TOOLBAR.SAVE"),
    icon: "lucide-save",
    permissions: ["webhooks:update"],
    disabled: computed(() => !form.canSave.value),
    clickHandler: () => save(),
  },
  {
    id: "reset",
    title: t("WEBHOOKS.TOOLBAR.RESET"),
    icon: "lucide-undo-2",
    disabled: computed(() => !form.isModified.value),
    clickHandler: async () => {
      form.revert();
      await loadProperties(eventId.value);
    },
  },
  {
    id: "delete",
    title: t("WEBHOOKS.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: ["webhooks:delete"],
    isVisible: computed(() => !isNew.value),
    async clickHandler() {
      if (await showConfirmation(t("WEBHOOKS.DETAILS.DELETE_CONFIRM", { name: stored.value.name }))) {
        await api.delete([param.value!]);
        form.setBaseline();
        await callParent("reload");
        await closeSelf();
      }
    },
  },
]);

// the errors blade refreshes the badge after it deletes feed records
exposeToChildren({ reload: () => countErrors() });

load();
</script>

<style lang="scss">
.webhook-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 20px 22px 36px;
  &__note {
    margin-top: 4px;
  }
}

.webhook-card {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 16px;
}
</style>
