<template>
    <div class="text-h6">Authorization</div>
    <v-radio-group inline v-model="authorizationType.value.value" :error-messages="authorizationType.errorMessage.value">
        <v-radio label="None" value="None"></v-radio>
        <v-radio label="Authorized" value="Authorized"></v-radio>
    </v-radio-group>
    <v-select v-show="authorizationType.value.value === 'Authorized'" label="Policies" :items="policies" multiple>

    </v-select>
</template>
<script setup lang="ts">
import { useField, useForm } from 'vee-validate';
import * as yup from 'yup';

interface Inputs {
    type: string;
    policies: string[];
}
interface Policy {
    name?: string;
    expression?: string;
    template_id?: number;
}

const authorizationType = useField('authorizationType');
const policies = ['Foo', 'Bar'];

const schema = yup.object({
    name: yup.string().required('Route name is required'),
    path: yup.string().required('Path is required').matches(/^\/.*/, 'Path must start with "/"'),
});
const { handleSubmit } = useForm<Inputs>({
    validationSchema: schema,
    initialValues: {
        type: 'None',
        policies: [],
    }
});

</script>