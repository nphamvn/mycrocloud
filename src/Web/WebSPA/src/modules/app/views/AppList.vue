<template>
    <div class="h-full max-w-screen-lg mx-auto p-2">
        <div class="mt-4 px-3 flex flex-row">
            <h1 class="text-lg font-semibold">Your Apps</h1>
            <RouterLink :to="{ name: 'AppCreate' }" class="ms-auto">
                <FwbButton>New</FwbButton>
            </RouterLink>
        </div>
        <form class="mt-3 flex items-center">
            <label for="simple-search" class="sr-only">Search</label>
            <input type="text" id="simple-search" v-model="searchTerm"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5  dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                placeholder="Search apps...">
        </form>
        <div v-if="!loading" class="mt-5">
            <div v-if="apps.length === 0">You don't have any app. Create one.</div>
            <ul v-else role="list" class="divide-y">
                <li role="listitem" v-for="app in apps" :key="app.id" class="p-2">
                    <div class="pb-3">
                        <h3 class="text-lg text font-semibold text-blue-500">
                            <RouterLink :to="`/app/${app.id}`">{{ app.name }}</RouterLink>
                        </h3>
                        <p class="text-sm">{{ app.description }}</p>
                        <p class="text-sm text-slate-500">Created {{ moment(app.createdAt).fromNow() }}</p>
                    </div>
                </li>
            </ul>
        </div>
    </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { watch } from 'vue';
import moment from 'moment';
import AppItem from '../models/AppItem';
import debounce from '@/modules/core/utils';
import { FwbButton } from 'flowbite-vue';
import { useAppStore } from '../store/appStore';

const { getApps } = useAppStore();

const loading = ref(true);

const apps = ref<AppItem[]>([])

onMounted(async () => {
    await fetchApps(undefined);
})

const searchTerm = ref();
const debouncedSearch = debounce(async () => {
    await fetchApps(searchTerm.value);
}, 1000);

watch(searchTerm, () => {
    debouncedSearch();
})

async function fetchApps(searchTerm: string | undefined) {
    apps.value = await getApps();
    loading.value = false;
}
</script>