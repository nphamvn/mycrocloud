<script setup lang="ts">
import { onMounted, ref } from 'vue';
import AppItem from '../AppItem';
import { watch } from 'vue';
import debounce from '../helper';
import moment from 'moment';

const loading = ref(true);

const apps = ref<AppItem[]>([])

onMounted(() => {
    fetchApps(undefined);
})

const searchTerm = ref();
const debouncedSearch = debounce(() => {
    fetchApps(searchTerm.value);
}, 1000);
watch(searchTerm, () => {
    debouncedSearch();
})

function fetchApps(searchTerm: string | undefined) {
    console.log('fetchApps:', searchTerm);
    setTimeout(() => {
        const items: AppItem[] = [];
        for (let index = 1; index <= 10; index++) {
            items.push({ id: index, name: `App ${index}`, description: `App ${index}`, createdAt: new Date() });
        }
        apps.value = items;
        loading.value = false;
    }, 1000)
}

</script>

<template>
    <div>
        <h1 class="text-lg font-bold">Your Apps</h1>
        <div class="flex mt-2">
            <div>
                <input type="search" v-model="searchTerm" class="p-2 border rounded-md focus:outline-none" placeholder="Search apps..." />
            </div>
            <div class="ms-auto">
                <RouterLink to="/apps/new" class="ms-auto">
                    <button type="button" class="p-2 bg-violet-600 text-white rounded-md">New</button>
                </RouterLink>
            </div>
        </div>
        <div v-if="!loading" class="mt-5">
            <div v-if="apps.length === 0">You don't have any app. Create one.</div>
            <ul v-else role="list" class="p-2">
                <li role="listitem" v-for="app in apps" :key="app.id" class="p-2">
                    <div class="pb-3">
                        <h3 class="text-lg text font-semibold text-blue-500"><RouterLink :to="`/apps/${app.id}`">{{ app.name }}</RouterLink></h3>
                        <p class="text-sm">{{ app.description }}</p>
                        <p class="text-sm text-slate-500">Created {{ moment(app.createdAt).fromNow() }}</p>
                    </div>
                </li>
            </ul>
        </div>
    </div>
</template>