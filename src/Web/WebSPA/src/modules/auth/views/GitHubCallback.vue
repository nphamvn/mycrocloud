<template>
    <h1>{{ isLoading ? 'Logging in...' : ''}}</h1>
    <RouterLink :to="{name: 'Login'}">Login</RouterLink>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';

const isLoading = ref(true);
const route = useRoute();

const code = route.query['code']?.valueOf();
const state = route.query['state']?.valueOf();
console.log({ code, state });
const storedState = localStorage.getItem('state');
if (state !== storedState) {
    window.location.href = '/';
}

try {
    const res = await fetch(`https://localhost:5024/auth/login/github?code=${code}`, {
        method: 'POST'
    });

    const authResult = await res.json();
    console.log(authResult);
    isLoading.value = false;
} catch (error) {
    console.error(error);
}


</script>