<template>
    <form v-on:submit="onSubmit" class="p-2">
        <div>
            <label for="name" class="block mb-1 text-sm font-medium text-gray-900">Name</label>
            <input type="text" v-model="name.value.value" id="name"
                class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2"
                placeholder="Enter route name..." aria-autocomplete="none">
            <span class="text-red-500">{{ name.errorMessage.value }}</span>
        </div>
        <div class="mt-2">
            <label class="block mb-1 text-sm font-medium text-gray-900">Method & Path</label>
            <div class="flex">
                <select v-model="method.value.value"
                    class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500">
                    <option v-for="method in ROUTE_METHODS" :key="method" :value="method">{{ method }}</option>
                </select>
                <input type="text" v-model="path.value.value"
                    class="ms-2 bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2" />
            </div>
            <span class="text-red-500" v-for="message in [method.errorMessage.value, path.errorMessage.value]">{{
                message }}</span>
        </div>
        <div class="mt-2">
            <label for="desciption" class="block mb-1 text-sm font-medium text-gray-900 ">Desciption</label>
            <textarea id="desciption" v-model="desciption.value.value" rows="2"
                class="block p-2 w-full text-sm text-gray-900 bg-gray-50 rounded-lg border border-gray-300 focus:ring-blue-500 focus:border-blue-500"></textarea>
        </div>
        <div>Response</div>
        <div class="mt-2">
            <label for="responseType" class="block mb-1 text-sm font-medium text-gray-900 ">Type</label>
            <div class="flex">
                <div class="flex items-center mr-4">
                    <input id="mockResponse" type="radio" v-model="responseType.value.value" value="mock"
                        :checked="responseType.value?.value === 'mock'" name="inline-radio-group"
                        class="w-4 h-4 bg-gray-100 border-gray-300">
                    <label for="mockResponse" class="ml-2 text-sm text-gray-900">Mock</label>
                </div>
                <div class="flex items-center mr-4">
                    <input type="radio" value="" name="inline-radio-group" class="w-4 h-4 bg-gray-100 border-gray-300"
                        disabled>
                    <label for="inline-2-radio" class="ml-2 text-sm text-gray-400">Proxied</label>
                </div>
            </div>
            <span class="text-red-500">{{ responseType.errorMessage.value }}</span>
        </div>
        <section v-if="openingRoute?.id !== undefined" type="button">

        </section>
        <button v-if="openingRoute?.id !== undefined" type="button" :disabled="openingRoute?.id === undefined"
            class="mt-2 focus:outline-none text-white bg-yellow-400 hover:bg-yellow-500 focus:ring-4 focus:ring-yellow-300 font-medium rounded-lg text-sm px-3 py-2 mr-2 mb-2"
            @click="router.push({ name: 'RouteResponse', params: { appId: 1, routeId: openingRoute?.id }, query: { type: openingRoute?.responseType } })">
            Configure
        </button>
        <hr class="mt-2">
        <div class="mt-2 d-flex">
            <button type="submit"
                class="ms-auto focus:outline-none text-white bg-purple-700 hover:bg-purple-800 focus:ring-4 focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 mb-2">Save</button>
        </div>
    </form>
</template>
<script setup lang="ts">
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';
import { useRouter } from "vue-router";
import { onMounted, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRouteStore } from '../../store/routeStore';
import { ROUTE_METHODS } from '../../constants';
const router = useRouter();
const store = useRouteStore();
const { setOpeningRoute, createRoute, getRouteById, updateRoute } = store;
const { openingRoute } = storeToRefs(store);

interface Inputs {
    name: string;
    method: string;
    path: string;
    responseType: string;
    desciption: string;
}

const schema = yup.object({
    name: yup.string().required('Route name is required'),
    path: yup.string().required('Path is required').matches(/^\/.*/, 'Path must start with "/"'),
});

const initialValues: Inputs = {
    name: 'Foo',
    method: ROUTE_METHODS[Math.floor(Math.random() * ROUTE_METHODS.length)],
    path: '/foo',
    desciption: 'Should return "bar"',
    responseType: 'mock',
}

const { handleSubmit, resetForm } = useForm<Inputs>({
    validationSchema: schema,
    initialValues: initialValues,
});
const onSubmit = handleSubmit(async (data) => {
    const routeId = openingRoute.value?.id;
    if (!routeId) {
        const newRoute = await createRoute(data);
        setOpeningRoute(newRoute);
        router.replace(`/app/1/route/${newRoute.id}`);
    }
    else {
        await updateRoute({ ...data, id: routeId });
    }
})

const name = useField<string>('name');
const path = useField<string>('path');
const method = useField<string>('method');
const desciption = useField<string>('desciption');
const responseType = useField<string>('responseType');

async function render() {
    const routeId = openingRoute.value?.id;
    if (routeId) {
        const route = await getRouteById(routeId);
        setOpeningRoute(route!);
        resetForm({ values: route });
    } else {
        resetForm({ values: initialValues });
    }
}

onMounted(async () => await render())

watch(openingRoute, async (newRoute, oldRoute) => {
    if (newRoute?.id != oldRoute?.id) await render();
})
</script>