<template>
    <div class="flex flex-row">
        <div>
            <div>
                <button @click="openRoute(undefined)" class="bg-violet-600 text-white px-x py-1 w-full rounded-md">New</button>
            </div>
            <div>
                <input type="search" class="border rounded-md px-2 py-1" placeholder="Search" />
            </div>
            <div class="mt-2">
                <ul>
                    <li v-for="route in routes"
                        :class="route.id === openingRoute?.id ? 'bg-violet-300' : ''" class="border-b">
                        <a href="#" v-on:click="(e) => { e.preventDefault(); openRoute(route.id)}" class="block">
                            <p class="text-sm font-semibold">{{ route.name }}</p>
                            <span 
                                class="text-xs uppercase text-white px-1 py-0 border rounded-md max-w-xs" 
                                :class="routeMethodBgColors.find(m => m.method === route.method)?.bgColor">
                                {{ route.method }}
                            </span>
                            <span class="ms-2 text-sm">{{ route.template }}</span>
                        </a>
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

const routeMethodBgColors = [
    { method: "GET", bgColor: "bg-blue-500"},
    { method: "POST", bgColor: "bg-green-500"},
    { method: "PUT", bgColor: "bg-cyan-400"},
    { method: "DELETE", bgColor: "bg-red-500"}
];

const routes = ref<RouteItem[]>([]);
const openingRoute = ref<RouteItem>();

onMounted(() => {
    routes.value = Array.from({ length: 10 }, (_, index) => ({
        id: index + 1,
        name: `Route ${index + 1}`,
        template: `/foo/${index + 1}`,
        method: 'PUT'
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