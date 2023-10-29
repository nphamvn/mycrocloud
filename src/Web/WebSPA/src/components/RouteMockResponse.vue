<template>
    <h4>Mock Response</h4>
    <form @submit="onSubmit">
        <div class="grid grid-cols-4">
            <div class="mt-2">
                <label for="statusCode" class="block mb-1 text-sm font-medium text-gray-900">Status Code</label>
                <input type="number" v-model="statusCode.value.value" id="statusCode"
                    class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2"
                    placeholder="Enter status code...">
                <span class="text-red-500">{{ statusCode.errorMessage.value }}</span>
            </div>
        </div>
        <div class="mt-2">
            <label for="statusCode" class="block mb-1 text-sm font-medium text-gray-900">Body</label>
            <select v-model="bodyLanguage.value.value" class="border border-gray-300 p-1 rounded-md focus:ring-transparent text-gray-900 text-sm">
                <option value="javascript">Javascript</option>
                <option value="json">Json</option>
            </select>
            <div ref="bodyEditorRef" style="min-height: 200px;" class="w-full border mt-1"></div>
            <span class="text-red-500">{{ body.errorMessage.value }}</span>
        </div>
        <div class="mt-2 d-flex">
            <button type="submit"
                class="ms-auto focus:outline-none text-white bg-purple-700 hover:bg-purple-800 focus:ring-4 focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 mb-2">Save</button>
        </div>
    </form>
</template>
<script setup lang="ts">
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';
import * as monaco from "monaco-editor";
import { onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { useRouteStore } from './routeStore';

const location = useRoute();
const routeId = parseInt(location.params['routeId'][0]);
const { getRouteById, setRouteResponse } = useRouteStore();

interface Inputs {
    statusCode: number,
    body: string,
    bodyLanguage: string
}

const schema = yup.object({
    statusCode: yup.number().required('Status code is required'),
    body: yup.string(),
    bodyLanguage: yup.string().oneOf(['json'])
});

const initialValues: Inputs = {
    statusCode: 200,
    body: '',
    bodyLanguage: 'json'
}

const { handleSubmit, resetForm } = useForm<Inputs>({
    validationSchema: schema,
    initialValues: initialValues,
});

const statusCode = useField<number>('statusCode');
const body = useField<string>('body');
const bodyLanguage = useField<string>('bodyLanguage');

const bodyEditorRef = ref();
let bodyEditor: monaco.editor.IStandaloneCodeEditor;
onMounted(async () => {
    const response = (await getRouteById(routeId))!.response;
    resetForm({ values: response});
    if (bodyEditorRef.value) {
        bodyEditor = monaco.editor.create(bodyEditorRef.value, {
            value: body.value.value,
            language: bodyLanguage.value.value
        });
        bodyEditor.onDidChangeModelContent(() => {
            body.setValue(bodyEditor.getValue())
            body.validate();
        });
    }
})
onBeforeUnmount(() => bodyEditor?.dispose());

watch(bodyLanguage.value, () => {
    const model = bodyEditor.getModel()!;
    monaco.editor.setModelLanguage(model, bodyLanguage.value.value)
})

const onSubmit = handleSubmit(async (data) => {
    await setRouteResponse(routeId, data);
})
</script>