import { App, Plugin } from "vue";

import AppLayout from "./AppLayout.vue";
import DefaultLayout from "../components/DefaultLayout.vue";
import DashboardLayout from "../modules/app/components/Layout.vue";
import PageLoader from "../components/PageLoader.vue";
import Spinner from "../components/Spinner.vue";

const DynamicLayoutPlugin: Plugin = {
  install(app: App) {
    app.component('Spinner', Spinner)
    app.component('PageLoader', PageLoader)
    app.component("AppLayout", AppLayout);
    app.component("DefaultLayout", DefaultLayout);
    app.component("DashboardLayout", DashboardLayout);
  },
};

export default DynamicLayoutPlugin;
