import AppLayout from "./AppLayout.vue";
import DefaultLayout from "../components/DefaultLayout.vue";
import DashboardLayout from "../modules/app/components/Layout.vue";
import { App, Plugin } from "vue";

const DynamicLayoutPlugin: Plugin = {
  install(app: App) {
    app.component("AppLayout", AppLayout);
    app.component("DefaultLayout", DefaultLayout);
    app.component("DashboardLayout", DashboardLayout);
  },
};

export default DynamicLayoutPlugin;
