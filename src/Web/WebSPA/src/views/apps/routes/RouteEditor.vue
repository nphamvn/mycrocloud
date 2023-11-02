<template>
    <form v-on:submit="onSubmit" class="p-2">
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


const schema = yup.object({
    name: yup.string().required('Route name is required'),
    path: yup.string().required('Path is required').matches(/^\/.*/, 'Path must start with "/"'),
});


const { handleSubmit } = useForm<Inputs>({
    validationSchema: schema,
    initialValues: {
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