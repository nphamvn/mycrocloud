import { defineStore } from "pinia";
import { ref } from "vue";

export const store  = defineStore('auth', () => {
    const isAuthenticated = ref(false);
    const user = ref
    const login = () => {
        isAuthenticated.value = true;
    }

    return { login, isAuthenticated }
});