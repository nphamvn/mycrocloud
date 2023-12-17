<template>
    <div class="flex flex-row w-full h-screen p-1">
        <div class="w-32 flex flex-col border p-1 h-full space-y-1">
            <RouterLink :to="{ name: 'Overview', params: { appId } }" class="text-xs p-1"
                :class="[currentRouteName === 'Overview' ? activeClasses : '']">
                Overview</RouterLink>
            <RouterLink :to="{ name: 'RouteView', params: { appId } }" class="text-xs p-1"
                :class="[currentRouteName === 'RouteView' ? activeClasses : '']">
                Routes</RouterLink>
            <RouterLink :to="{ name: 'RequestLogging', params: { appId } }" class="text-xs p-1"
                :class="[currentRouteName === 'RequestLogging' ? activeClasses : '']">
                Request Logging</RouterLink>
        </div>
        <div class="w-full h-full">
            <router-view></router-view>
        </div>
    </div>
</template>
<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { onMounted } from 'vue';
import { computed } from '@vue/reactivity';
import { useAppStore } from '../store/appStore';

const router = useRouter();
const currentRouteName = computed(() => router.currentRoute.value.matched[1].name);
const activeClasses = ['border', 'rounded', 'bg-slate-50'];

const location = useRoute();
const appId = parseInt(location.params['id'].toString());

const { setCurrentApp } = useAppStore();

onMounted(async () => {
    const app = await getAppById(appId)!;
    setCurrentApp(app);
    document.title = app.name;
});

const getAppById = async (id: number) => {
    return {
        id: id,
        name: "string",
        description: "string",
        createdAt: new Date(),
        routeCount: 100
    }
}
</script>