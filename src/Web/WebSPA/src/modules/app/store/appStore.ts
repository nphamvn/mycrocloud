import { defineStore } from "pinia";
import { ref } from "vue";
import App from "../models/App";

export const useAppStore = defineStore("app", () => {
  const apps = ref<App[]>([]);
  const currentApp = ref<App>();

  const fake_apps: App[] = Array.from({ length: 10 }, (_, index) => ({
    id: index + 1,
    name: `App ${index + 1}`,
    description: `App ${index + 1}`,
    createdAt: new Date(),
    routeCount: (index + 1) * 10
  }));

  const getApps = async (query: string) => {
    apps.value = fake_apps;
    return fake_apps;
  };

  const getAppById = async (id) => {
    return fake_apps.find(a => a.id === id);
  }

  const setCurrentApp = (app: App) => {
    currentApp.value = app;
  }

  return { getApps, apps, getAppById, currentApp, setCurrentApp };
});
