<template>
    <div class="flex flex-row w-full h-screen p-1">
        <div class="w-20 flex flex-col border p-1 h-full">
            <router-link :to="{ name: 'Overview', params: { appId } }" class="text-sm p-1"
                :class="[router.currentRoute.value.matched[1].name === 'Overview' ? ['border', 'rounded-md', ' bg-slate-50'] : '']">Overview</router-link>
            <router-link :to="{ name: 'RouteView', params: { appId }}" class="text-sm p-1"
                :class="[router.currentRoute.value.matched[1].name === 'RouteView' ? ['border', 'rounded-md', ' bg-slate-50'] : '']">Routes</router-link>
        </div>
        <div class="w-full h-full">
            <router-view></router-view>
        </div>
    </div>
</template>
<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { onMounted } from 'vue';
import { storeToRefs } from "pinia";
import { useAppStore } from '../store/appStore';

const router = useRouter();
const location = useRoute();
const appId = parseInt(location.params['id'].toString());

const store = useAppStore();
const { getAppById, setCurrentApp } = store;

const { currentApp: app } = storeToRefs(store);

onMounted(async () => {
    const app = await getAppById(appId)!;
    await setCurrentApp(app);
    document.title = app.name;
});

</script>