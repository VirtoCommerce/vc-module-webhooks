import path from "node:path";
import { getDynamicModuleConfiguration } from "@vc-shell/mf-module";

// Webhooks: a Module Federation remote of the Admin Shell host.
// Output: <module>/plugins/admin-shell/ (remoteEntry.js + public/plugin.json),
// served by the platform at /modules/$(VirtoCommerce.WebHooks)/plugins/admin-shell/.
const base = "/modules/$(VirtoCommerce.WebHooks)/plugins/admin-shell/";
process.env.APP_BASE_PATH = base;

export default getDynamicModuleConfiguration({
  appId: "admin-shell",
  moduleRoot: path.resolve(__dirname, ".."),
  entry: "./src/modules/index.ts",
  remoteName: "webhooks_admin",
  base,
});
