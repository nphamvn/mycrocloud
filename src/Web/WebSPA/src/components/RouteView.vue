<template>
    <div class="flex">
        <div>
            <div class="flex">
                <button @click="openRoute(undefined)" class="bg-violet-600 text-white px-4 py-2">New</button>
                <input type="search" placeholder="Search" />
            </div>
            <div>
                <ul>
                    <li v-for="route in routes"
                        :class="route.id === openingRoute?.id ? 'v-list-item-active' : ''"
                        v-on:click="openRoute(route.id)">{{ route.name }}
                    </li>
                </ul>
            </div>
        </div>
        <div class="border">
            <RouteEditor v-if="openingRoute" />
        </div>
    </div>
</template>
<script setup lang="ts" >
import { onMounted } from 'vue';
import { ref } from 'vue';
import RouteEditor from './RouteEditor.vue';

interface RouteItem {
    id?: number;
    name?: string;
    template?: string;
    method?: string;
}

const routes = ref<RouteItem[]>([]);
const openingRoute = ref<RouteItem>();

onMounted(() => {
    routes.value = Array.from({ length: 10 }, (_, index) => ({
        id: index + 1,
        name: `Route ${index + 1}`,
        template: '',
        method: 'GET'
    }))
})

const openRoute = (id: number | undefined) => {
    if (id) {
        openingRoute.value = routes.value.find(r => r.id === id);
    }
    else {
        openingRoute.value = {
            name: 'Foo',
            template: '/foo',
            method: 'ANY'
        };
    }
}

</script>
<style scoped>
.v-list-item-active {
    background-color: aliceblue;
}
</style>