import { defineStore } from "pinia";
import { ref } from "vue";
import App from "../models/App";

export const useAppStore = defineStore("app", () => {
  //const apps = ref<App[]>([]);
  const currentApp = ref<App>();

  // const fake_apps: App[] = Array.from({ length: 10 }, (_, index) => ({
  //   id: index + 1,
  //   name: `App ${index + 1}`,
  //   description: `App ${index + 1}`,
  //   createdAt: new Date(),
  //   routeCount: (index + 1) * 10
  // }));

  const setCurrentApp = (app: App) => {
    currentApp.value = app;
  }

  return { currentApp, setCurrentApp };
});
