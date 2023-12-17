<template>
    <div class="flex flex-row h-full">
        <div class="basis-1/5 border">
            <div class="p-1">
                <FwbButton size="sm" class="w-full" @click="newRoute">New</FwbButton>
            </div>
            <div class="p-1">
                <input type="search"
                    class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-1"
                    placeholder="Search" />
            </div>
            <div class="mt-2">
                <ul role="list">
                    <li v-for="route in routes" :class="route.id === openingRoute?.id ? 'bg-slate-200' : ''"
                        class="border-b hover:bg-slate-100 hover:cursor-pointer route-item"
                        v-on:click="openRoute(route.id)">
                        <div class="flex flex-row p-1">
                            <span class="inline-flex items-center rounded-md px-1 text-xs text-white leading-6"
                                :class="getRouteMethodBgColors(route.method)">
                                {{ route.method }}</span>
                            <span class="ms-1 inline-block align-middle text-sm my-auto text-slate-900 leading-6">{{
                                route.name
                            }}</span>
                            <span class="ms-auto my-auto align-middle route-item-menu-button"
                                @click="(e) => showMenu(e, route)">
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"
                                    class="w-6 h-6">
                                    <path fill-rule="evenodd"
                                        d="M4.5 12a1.5 1.5 0 113 0 1.5 1.5 0 01-3 0zm6 0a1.5 1.5 0 113 0 1.5 1.5 0 01-3 0zm6 0a1.5 1.5 0 113 0 1.5 1.5 0 01-3 0z"
                                        clip-rule="evenodd" />
                                </svg>
                            </span>
                        </div>
                    </li>
                </ul>
                <!-- Dropdown menu -->
                <template v-if="showDropdownMenu">
                    <div ref="dropdownMenu" class="z-10 bg-white divide-y divide-gray-100 rounded-lg shadow w-44">
                        <ul class="py-2 text-sm text-gray-700" aria-labelledby="dropdownButton">
                            <li>
                                <a href="#" @click="handleDeleteButtonClick"
                                    class="block px-4 py-2 hover:bg-gray-100 text-red-500">Delete</a>
                            </li>
                        </ul>
                    </div>
                </template>
                <RouteDeleteConfirmModal v-if="showDeleteConfirmModal" @on-delete-click="handleModalDeleteButtonClick"
                    @on-cancle-click="handleModalCancleButtonClick" @on-hide="showDeleteConfirmModal = false">
                </RouteDeleteConfirmModal>
            </div>
        </div>
        <div class="basis-4/5 border p-2 h-[600]">
            <router-view></router-view>
        </div>
    </div>
</template>
<style scoped>
.route-item:not(:hover) .route-item-menu-button {
    display: none;
}
</style>
<script setup lang="ts" >
import { ref, nextTick, onMounted, watch } from 'vue';
import { storeToRefs } from 'pinia'
import RouteDeleteConfirmModal from './RouteDeleteConfirmModal.vue';
import { FwbButton } from 'flowbite-vue';
import { useRoute, useRouter } from 'vue-router';
import RouteItem from '../../models/RouteItem';
import { useRouteStore } from '../../store/routeStore';
import { Dropdown } from "flowbite";
import { DropdownOptions, DropdownInterface } from "flowbite";

const router = useRouter();
const location = useRoute();
const store = useRouteStore();
const { getRoutes, setOpeningRoute, deleteRoute } = store;
const { routes, openingRoute } = storeToRefs(store);

const routeMethodBgColors = [
    { method: "ANY", bgColor: "bg-blue-500" },
    { method: "GET", bgColor: "bg-blue-500" },
    { method: "POST", bgColor: "bg-green-500" },
    { method: "PUT", bgColor: "bg-cyan-400" },
    { method: "DELETE", bgColor: "bg-red-500" },
    { method: "PACTH", bgColor: "bg-red-500" }
];

const getRouteMethodBgColors = (method: string) => {
    return routeMethodBgColors.find(m => m.method === method)?.bgColor;
}

watch(openingRoute, () => {
    if (openingRoute.value) {
        if (openingRoute.value.id) {
            router.push(`/app/1/route/${openingRoute.value.id}`);
        } else {
            router.push('/app/1/route/new');
        }
    }
})

const newRoute = () => {
    setOpeningRoute({} as RouteItem)
}
const openRoute = (id: number) => {
    if (id != openingRoute.value?.id) {
        setOpeningRoute({ id: id } as RouteItem);
    }
}

onMounted(async () => {
    await getRoutes();
    const routeId = location.params['routeId'] as string;
    if (routeId) {
        setOpeningRoute({ id: parseInt(routeId) } as RouteItem);
    }
});

//#region PopupMenu

const showDropdownMenu = ref(false);
const dropdownMenu = ref()
const showDeleteConfirmModal = ref(false);
const menuRoute = ref<RouteItem>();

const showMenu = async (e: MouseEvent, route: RouteItem) => {
    e.stopPropagation();
    showDropdownMenu.value = true;
    menuRoute.value = route;
    let dropdown: DropdownInterface;
    const dropdownOptions: DropdownOptions = {
        placement: 'bottom',
        triggerType: 'click',
        offsetSkidding: 0,
        offsetDistance: 10,
        delay: 300,
        onHide: () => {
            console.log('dropdown has been hidden');
            dropdown.destroyAndRemoveInstance();
            showDropdownMenu.value = false;            
        },
        onShow: () => {
            console.log('dropdown has been shown');
        },
        onToggle: () => {
            console.log('dropdown has been toggled');
        }
    };

    await nextTick();
    dropdown = new Dropdown(dropdownMenu.value, e.target, dropdownOptions);
    dropdown.show();
}

const handleDeleteButtonClick = (e) => {
    console.log('handleDeleteButtonClick');
    e.preventDefault();
    showDeleteConfirmModal.value = true;
}

const handleModalDeleteButtonClick = async () => {
    console.log('handleModalDeleteButtonClick');
    await deleteRoute(menuRoute.value!.id);
    showDeleteConfirmModal.value = false;
}

const handleModalCancleButtonClick = () => {
    console.log('handleModalCancleButtonClick');
    showDeleteConfirmModal.value = false;
}
//#endregion
</script>