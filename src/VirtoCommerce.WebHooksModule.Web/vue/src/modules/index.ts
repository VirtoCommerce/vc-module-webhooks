import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule } from "@vc-shell/framework";

// Webhooks: the AngularJS module's list (main menu), details, and the errors widget + list on the details
export default defineAppModule({
  blades,
  locales,
});

export * from "./pages";
