<template>
    <div class="flex flex-row h-full">
        <div class="basis-1/5 border">
            <div class="p-1">
                <button type="button" @click="newRoute" class="bg-violet-600 text-white py-1 w-full rounded-md">New</button>
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
                            <span class="inline-flex items-center rounded-md px-1 text-xs text-white"
                                :class="routeMethodBgColors.find(m => m.method === route.method)?.bgColor">{{ route.method
                                }}</span>
                            <span class="ms-1 inline-block align-middle text-sm my-auto text-slate-900">{{ route.name
                            }}</span>
                            <span class="ms-auto my-auto align-middle route-item-menu-button"
                                @click="(e) => showMenu(e, route.id)">
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
                                <a ref="dropdownMenu_DeleteButton" href="#"
                                    class="block px-4 py-2 hover:bg-gray-100 text-red-500">Delete</a>
                            </li>
                        </ul>
                    </div>
                    <div ref="dropdownMenu_DeleteConfirmModal" tabindex="-1"
                        class="fixed top-0 left-0 right-0 z-50 hidden p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-[calc(100%-1rem)] max-h-full">
                        <div class="relative w-full max-w-md max-h-full">
                            <div class="relative bg-white rounded-lg shadow">
                                <div class="p-6 text-center">
                                    <h3 class="mb-5 text-lg font-normal text-gray-500">Are you sure you
                                        want to delete this route?</h3>
                                    <button ref="dropdownMenu_DeleteConfirmModal_DeleteButton" type="button"
                                        class="text-white bg-red-600 hover:bg-red-800 focus:ring-4 focus:outline-none focus:ring-red-300 font-medium rounded-lg text-sm inline-flex items-center px-5 py-2.5 text-center mr-2">
                                        Delete
                                    </button>
                                    <button ref="dropdownMenu_DeleteConfirmModal_CancelButton" type="button"
                                        class="text-gray-500 bg-white hover:bg-gray-100 focus:ring-4 focus:outline-none focus:ring-gray-200 rounded-lg border border-gray-200 text-sm font-medium px-5 py-2.5 hover:text-gray-900 focus:z-10">Cancel</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
            </div>
        </div>
        <div class="basis-4/5 border p-2 h-[600]">
            <router-view></router-view>
        </div>
    </div>
</template>
<style scoped>
.route-item:not(:hover) .route-item-menu-button {
    /* display: none; */
}
</style>
<script setup lang="ts" >
import { nextTick, onMounted, watch } from 'vue';
import { ref } from 'vue';
const router = useRouter();
const location = useRoute();
const routeId = location.params['routeId'] as string;
console.log('routeId', routeId);

const routeMethodBgColors = [
    { method: "ANY", bgColor: "bg-blue-500" },
    { method: "GET", bgColor: "bg-blue-500" },
    { method: "POST", bgColor: "bg-green-500" },
    { method: "PUT", bgColor: "bg-cyan-400" },
    { method: "DELETE", bgColor: "bg-red-500" },
    { method: "PACTH", bgColor: "bg-red-500" }
];

import { storeToRefs } from 'pinia'
const store = useRouteStore();
const { getRoutes, setOpeningRoute } = store;
const { routes, openingRoute } = storeToRefs(store);

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
    if (routeId) {
        setOpeningRoute({ id: parseInt(routeId) } as RouteItem);
    }
});


//#region PopupMenu
import { Dropdown, ModalInterface } from "flowbite";
import { DropdownOptions, DropdownInterface } from "flowbite";
const showDropdownMenu = ref(false);
const dropdownMenu = ref()
const dropdownMenu_DeleteButton = ref();
const dropdownMenu_DeleteConfirmModal = ref();

const dropdownMenu_DeleteConfirmModal_DeleteButton = ref();
const dropdownMenu_DeleteConfirmModal_CancelButton = ref();

const initMenu = (id) => {
    dropdownMenu_DeleteButton.value.addEventListener('click', (e) => { e.preventDefault(); showDeleteConfirmModal(id) })
}

import { Modal, ModalOptions } from 'flowbite';
import { useRoute, useRouter } from 'vue-router';
import { useRouteStore } from './routeStore';
import RouteItem from './RouteItem';
const showDeleteConfirmModal = (id) => {
    console.log('delete route: ', id);
    let modal: ModalInterface;
    const options: ModalOptions = {
        placement: 'top-center',
        backdrop: 'dynamic',
        backdropClasses: 'bg-gray-900 bg-opacity-50 dark:bg-opacity-80 fixed inset-0 z-40',
        closable: true,
        onHide: () => {
            console.log('modal is hidden');
            //dropdownMenu_DeleteConfirmModal_DeleteButton.value.removeEventListener('click');
            //dropdownMenu_DeleteConfirmModal_CancelButton.value.removeEventListener('click');
            modal.destroyAndRemoveInstance();
        },
        onShow: () => {
            console.log('modal is shown');
        },
        onToggle: () => {
            console.log('modal has been toggled');
        }
    };
    modal = new Modal(dropdownMenu_DeleteConfirmModal.value, options);
    dropdownMenu_DeleteConfirmModal_DeleteButton.value.addEventListener('click', () => {
        console.log('dropdownMenu_DeleteConfirmModal_DeleteButton')
    });
    dropdownMenu_DeleteConfirmModal_CancelButton.value.addEventListener('click', () => {
        console.log('dropdownMenu_DeleteConfirmModal_CancelButton')
    });
    modal.show();
}
const showMenu = async (e, id) => {
    e.stopPropagation();
    showDropdownMenu.value = true;
    let dropdown: DropdownInterface;
    const hide = () => {
        dropdown.hide();
        window.removeEventListener('mousedown', hide)
    }
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
    initMenu(id);
    //window.addEventListener('mousedown', hide)
    dropdown.show();
}
//#endregion
</script>