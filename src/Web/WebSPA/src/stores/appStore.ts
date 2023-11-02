import { defineStore } from "pinia";
import { ref } from "vue";
import App from "./App";

export const useAppStore = defineStore("app", () => {
  const apps = ref<App[]>([]);
  const currentApp = ref<App>();

  const fake_apps = Array.from({ length: 10 }, (_, index) => ({
    id: index + 1,
    name: `App ${index + 1}`,
    description: `App ${index + 1}`,
    createdAt: new Date(),
  }));

  const getApps = async () => {
    apps.value = fake_apps;
  };

  const getAppById = async (id) => {
    return fake_apps.find(a => a.id === id);
  }

  const setCurrentApp = (app: App) => {
    currentApp.value = app;
  }

  return { getApps, apps, getAppById, currentApp, setCurrentApp};
});
