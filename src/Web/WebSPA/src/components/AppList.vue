<script setup lang="ts">
import { onBeforeMount, ref } from 'vue';
import AppItem from '../AppItem';

const apps = ref<AppItem[]>([])


onBeforeMount(() => {
    setTimeout(() => {
        const items: AppItem[] = [];
        for (let index = 1; index <= 10; index++) {
            items.push({ id: index, name: `App ${index}` });
        }
        apps.value = items;
    }, 1000)
})

</script>

<template>
    <h1>Your Apps</h1>
    <RouterLink to="/apps/new">Create new</RouterLink>
    <div v-if="apps?.length === 0">You don't have any app. Create one.</div>
    <ul v-else>
        <li v-for="app in apps" :key="app.id">{{ app.name }}</li>
    </ul>
    <router-view></router-view>
</template>