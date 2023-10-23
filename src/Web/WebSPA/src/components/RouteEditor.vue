<template>
    <v-form v-on:submit="onSubmit">
        <section>
            <div class="text-h6">Basic Info</div>
            <v-text-field v-model="name.value.value" label="Name" aria-autocomplete="none"
                :error-messages="name.errorMessage.value">
            </v-text-field>
            <v-text-field v-model="path.value.value" label="Path" :error-messages="path.errorMessage.value">
            </v-text-field>
            <v-select v-model="method.value.value" label="Method" :items="methodItems"
                :error-messages="method.errorMessage.value">
            </v-select>
        </section>
        <section v-if="false">
            <div class="text-h6">Authorization</div>
            <v-radio-group inline v-model="authorizationType.value.value"
                :error-messages="authorizationType.errorMessage.value">
                <v-radio label="None" value="None"></v-radio>
                <v-radio label="Authorized" value="Authorized"></v-radio>
            </v-radio-group>
            <v-select v-show="authorizationType.value.value === 'Authorized'" label="Policies" :items="policies" multiple>

            </v-select>
        </section>
        <section>
            <div class="text-h6">Validation</div>
            <v-tabs v-model="validationTab">
                <v-tab value="QueryParams">Query Params</v-tab>
                <v-tab value="Headers">Headers</v-tab>
                <v-tab value="Body">Body</v-tab>
            </v-tabs>
            <v-window v-model="validationTab" class="pa-2">
                <v-window-item value="QueryParams">
                    <v-table>
                        <thead>
                            <tr>
                                <th class="text-left">
                                    Key
                                </th>
                                <th class="text-left">
                                    Rules
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in queryParamValidationSchemes.value.value" :key="index">
                                <td>
                                    {{ item.name }}
                                </td>
                                <td>{{ item.rules.length }}</td>
                            </tr>
                        </tbody>
                    </v-table>
                    <v-btn @click="onAddValidationRule('QueryParams')">Add</v-btn>
                </v-window-item>

                <v-window-item value="Headers">
                    Headers
                </v-window-item>

                <v-window-item value="Body">
                    Body
                </v-window-item>
            </v-window>
        </section>
        <div class="d-flex">
            <v-btn type="submit" class="ms-auto">Save</v-btn>
        </div>
    </v-form>
</template>
<script setup lang="ts">
import { useField, useForm } from 'vee-validate';
import { ref } from 'vue';
import * as yup from 'yup';

interface QueryParamValidationScheme {
    name: string;
    rules: string[]
}
interface Inputs {
    name: string;
    path: string;
    method: string;
    authorizationType: string;
    authorizationPolicies: string[];
    queryParamValidationSchemes: QueryParamValidationScheme[];
}

const methodItems = ['ANY', 'GET', 'POST', 'PUT', 'PACTH', 'DELETE'];
const policies = ['Foo', 'Bar'];
const validationTab = ref('QueryParams')

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
        queryParamValidationSchemes: []
    }
});
const onSubmit = handleSubmit(data => {
    alert(JSON.stringify(data));
})

const name = useField('name');
const path = useField('path');
const method = useField('method');
const authorizationType = useField('authorizationType');
const queryParamValidationSchemes = useField<QueryParamValidationScheme[]>('queryParamValidationSchemes');

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