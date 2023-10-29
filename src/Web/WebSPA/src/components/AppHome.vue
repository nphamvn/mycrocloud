<template>
    <Breadcrumb :items="[{ title: 'Apps', link: '/apps' }, { title: app?.name!, link: `#zzz` }]" divider="/"></Breadcrumb>
    <div class="mt-4 flex w-full h-[calc(100%-100px)]">
        <div class="w-20 flex flex-col border p-1 h-full">
            <router-link :to="`/app/${appId}`" class="text-sm p-1" exact-active-class="border rounded-md bg-slate-50">Overview</router-link>
            <router-link :to="`/app/${appId}/route`" class="text-sm p-1" exact-active-class="border rounded-md bg-slate-50">Routes</router-link>
            <p>{{ router.currentRoute.value.matched[1].name }}</p>
        </div>
        <div class="w-full h-full">
            <router-view></router-view>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import AppItem from '../AppItem';
import { onMounted } from 'vue';
import Breadcrumb from './Breadcrumb.vue';
const router = useRouter();
const location = useRoute();
const appId = parseInt(location.params['id'].toString());
const app = ref<AppItem>();

onMounted(() => {
    app.value = {
        id: appId,
        name: `App ${appId}`,
        createdAt: new Date()
    }
    document.title = app.value.name;
});

</script>