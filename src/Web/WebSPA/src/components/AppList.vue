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
    <v-container>
        <h1>Your Apps</h1>
        <v-row>
            <v-col>
                <v-text-field type="search" v-model="searchTerm" placeholder="Search apps..."></v-text-field>
            </v-col>
            <v-col cols="1">
                <RouterLink to="/apps/new" class="ms-auto">
                    <v-btn color="primary">New</v-btn>
                </RouterLink>
            </v-col>
        </v-row>
        <div v-if="!loading" class="mt-5">
            <div v-if="apps.length === 0">You don't have any app. Create one.</div>
            <v-list v-else>
                <v-list-item v-for="app in apps" :key="app.id">
                    <div class="pb-3">
                        <div class="text-h5">
                            <RouterLink :to="`/apps/${app.id}`" class="">{{ app.name }}</RouterLink>
                        </div>
                        <p class="text-medium-emphasis">{{ app.description }}</p>
                        <p class="text-medium-emphasis">Created {{ moment(app.createdAt).fromNow() }}</p>
                    </div>
                    <v-divider></v-divider>
                </v-list-item>
            </v-list>
        </div>
    </v-container>
</template>