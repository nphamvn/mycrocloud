<template>
    <form v-on:submit="onSubmit" class="p-2">
        <section v-if="enabledSections.basic">
            <div class="mt-2">
                <label for="name" class="block mb-1 text-sm font-medium text-gray-900">Name</label>
                <input type="text" v-model="name.value.value" id="name"
                    class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2"
                    placeholder="Enter route name..." aria-autocomplete="none">
                <span class="text-red-500">{{ name.errorMessage.value }}</span>
            </div>
            <div class="mt-2">
                <label class="block mb-1 text-sm font-medium text-gray-900">Method and Path</label>
                <div class="flex">
                    <select v-model="method.value.value"
                        class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500">
                        <option v-for="method in methodItems" :key="method" :value="method">{{ method }}</option>
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
        </section>
        <section v-if="enabledSections.authorization">
            <div class="text-h6">Authorization</div>
            <v-radio-group inline v-model="authorizationType.value.value"
                :error-messages="authorizationType.errorMessage.value">
                <v-radio label="None" value="None"></v-radio>
                <v-radio label="Authorized" value="Authorized"></v-radio>
            </v-radio-group>
            <v-select v-show="authorizationType.value.value === 'Authorized'" label="Policies" :items="policies" multiple>

            </v-select>
        </section>
        <section v-if="enabledSections.validation" class="mt-4">
            <div class="text-lg font-semibold">Validation</div>
            <div class="mb-4 border-b border-gray-200">
                <ul class="flex flex-wrap -mb-px text-sm font-medium text-center" id="default-tab"
                    data-tabs-toggle="#default-tab-content" role="tablist">
                    <li class="mr-2" role="presentation">
                        <button class="inline-block p-4 border-b-2 rounded-t-lg" id="QueryParams-tab"
                            data-tabs-target="#QueryParams" type="button" role="tab" aria-controls="QueryParams"
                            aria-selected="false">Query Params</button>
                    </li>
                    <li class="mr-2" role="presentation">
                        <button
                            class="inline-block p-4 border-b-2 rounded-t-lg hover:text-gray-600 hover:border-gray-300 dark:hover:text-gray-300"
                            id="Headers-tab" data-tabs-target="#Headers" type="button" role="tab" aria-controls="Headers"
                            aria-selected="false">Headers</button>
                    </li>
                    <li class="mr-2" role="presentation">
                        <button
                            class="inline-block p-4 border-b-2 rounded-t-lg hover:text-gray-600 hover:border-gray-300 dark:hover:text-gray-300"
                            id="Body-tab" data-tabs-target="#Body" type="button" role="tab" aria-controls="Body"
                            aria-selected="false">Body</button>
                    </li>
                </ul>
            </div>
            <div id="default-tab-content">
                <div class="hidden p-2 rounded-lg" id="QueryParams" role="tabpanel" aria-labelledby="QueryParams-tab">
                    <div class="relative overflow-x-auto">
                        <table class="w-full text-sm text-left text-gray-500">
                            <thead class="text-xs text-gray-700 uppercase bg-gray-50">
                                <tr>
                                    <th scope="col" class="p-3 w-1/5">
                                        Name
                                    </th>
                                    <th scope="col" class="p-3">
                                        Rules
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="bg-white border-b">
                                    <td scope="row" class="w-1/5 p-3 font-medium text-gray-900 whitespace-nowrap">
                                        Foo
                                    </td>
                                    <td class="p-3 font-medium text-gray-900 whitespace-nowrap">
                                        <div class="flex flex-auto">
                                            <span v-for="i in 5">Required {{ i }}</span>
                                            <button>Add</button>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="hidden p-2 rounded-lg " id="Headers" role="tabpanel" aria-labelledby="Headers-tab">
                </div>
                <div class="hidden p-2 rounded-lg" id="Body" role="tabpanel" aria-labelledby="Body-tab">
                </div>
            </div>
        </section>
        <div class="mt-2 d-flex">
            <button type="submit"
                class="ms-auto focus:outline-none text-white bg-purple-700 hover:bg-purple-800 focus:ring-4 focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 mb-2">Save</button>
        </div>
    </form>
</template>
<script setup lang="ts">
import { initFlowbite } from 'flowbite';
import { useField, useForm } from 'vee-validate';
import { onMounted } from 'vue';
import * as yup from 'yup';

const enabledSections = {
    basic: true,
    authorization: false,
    validation: false,
    response: true
}

interface QueryParamValidationScheme {
    name: string;
    rules: string[]
}
interface Response {
    type: string;
    props: ResponseProps;
}
interface ResponseProps {

}
interface MockResponseProps extends ResponseProps {
    statusCode: number;
    body: string;
}
interface Inputs {
    name: string;
    method: string;
    path: string;
    desciption: string;
    authorizationType: string;
    authorizationPolicies: string[];
    queryParamValidationSchemes: QueryParamValidationScheme[];
    response: Response;
}

const methodItems = ['ANY', 'GET', 'POST', 'PUT', 'PACTH', 'DELETE'];
const policies = ['Foo', 'Bar'];

const schema = yup.object({
    name: yup.string().required('Route name is required'),
    path: yup.string().required('Path is required').matches(/^\/.*/, 'Path must start with "/"'),
});


const { handleSubmit } = useForm<Inputs>({
    validationSchema: schema,
    initialValues: {
        name: 'Foo',
        path: '/foo',
        method: methodItems[0],
        authorizationType: 'None',
        authorizationPolicies: [],
        queryParamValidationSchemes: [],
        response: {
            type: 'mock',
            props: {
                statusCode: 200
            } as MockResponseProps
        }
    }
});
const onSubmit = handleSubmit(data => {
    alert(JSON.stringify(data));
})

const name = useField<string>('name');
const path = useField<string>('path');
const method = useField<string>('method');
const desciption = useField<string>('desciption');
const authorizationType = useField('authorizationType');
const queryParamValidationSchemes = useField<QueryParamValidationScheme[]>('queryParamValidationSchemes');

onMounted(() => initFlowbite());
const onAddValidationRule = (prop: string) => {
    switch (prop) {
        case 'QueryParams':
            queryParamValidationSchemes.value.value.push({ name: '', rules: [] })
            break;

        default:
            break;
    }
}

</script>