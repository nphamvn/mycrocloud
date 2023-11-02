import { defineStore } from "pinia";
import { ref } from "vue";
import { User } from "./user";

export const store  = defineStore('auth', () => {
    const isAuthenticated = ref(false);
    const user = ref<User>();
    const login = () => {
        isAuthenticated.value = true;
    }

    return { login, isAuthenticated, user }
});