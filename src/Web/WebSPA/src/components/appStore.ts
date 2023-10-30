import { defineStore } from "pinia";
import { ref } from "vue";
import App from "./App";

export const useAppStore = defineStore("app", () => {
  const apps = ref<App[]>([]);
  const currentApp = ref<App>();

  const getApps = async () => {
    apps.value = Array.from({ length: 10 }, (_, index) => ({
      id: index + 1,
      name: `App ${index + 1}`,
      description: `App ${index + 1}`,
      createdAt: new Date(),
    }));
  };

  const getAppById = async (id) => {
    return apps.value.find(a => a.id === id);
  }

  const setCurrentApp = (app: App) => {
    currentApp.value = app;
  }

  return { getApps, apps, getAppById, currentApp, setCurrentApp};
});
