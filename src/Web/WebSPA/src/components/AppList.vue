<script setup lang="ts">
import { onBeforeMount, ref } from 'vue';
import AppItem from '../AppItem';

const apps = ref<AppItem[]>([])

onBeforeMount(() => {
    setTimeout(() => {
        const items: AppItem[] = [];
        for (let index = 1; index <= 10; index++) {
            items.push({ id: index, name: `App ${index}`, description: `App ${index}` });
        }
        apps.value = items;
    }, 1000)
})

</script>

<template>
    <v-container>
        <h1>Your Apps</h1>
        <v-row>
            <v-col>
                <v-text-field type="search" placeholder="Search apps..."></v-text-field>
            </v-col>
            <v-col cols="1">
                <RouterLink to="/apps/new" class="ms-auto">
                    <v-btn color="primary">New</v-btn>
                </RouterLink>
            </v-col>
        </v-row>
        <div class="mt-5">
            <div v-if="apps.length === 0">You don't have any app. Create one.</div>
            <v-list v-else>
                <v-list-item v-for="app in apps" :key="app.id">
                    <div class="pb-3">
                        <div class="text-h5">
                            <RouterLink :to="`/apps/${app.id}`" class="">{{ app.name }}</RouterLink>
                        </div>
                        <p class="text-medium-emphasis">{{ app.description }}</p>
                    </div>
                    <v-divider></v-divider>
                </v-list-item>
            </v-list>
        </div>
    </v-container>
</template>