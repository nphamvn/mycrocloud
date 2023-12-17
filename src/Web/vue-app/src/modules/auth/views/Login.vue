<template>
    <section class="bg-gray-50 dark:bg-gray-900">
        <div class="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0">
            <div
                class="w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
                <div class="p-6 space-y-4 md:space-y-6 sm:p-8">
                    <h1 class="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
                        Sign in to your account
                    </h1>
                    <form class="space-y-4 md:space-y-6" method="post" action="#">
                        <div>
                            <label for="email" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Your
                                email</label>
                            <input type="email" name="email" id="email"
                                class="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                placeholder="name@company.com">
                        </div>
                        <div>
                            <label for="password"
                                class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Password</label>
                            <input type="password" name="password" id="password" placeholder="••••••••"
                                class="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500">
                        </div>
                        <div class="flex items-center justify-between">
                            <div class="flex items-start">
                                <div class="flex items-center h-5">
                                    <input id="remember" aria-describedby="remember" type="checkbox"
                                        class="w-4 h-4 border border-gray-300 rounded bg-gray-50 focus:ring-3 focus:ring-primary-300 dark:bg-gray-700 dark:border-gray-600 dark:focus:ring-primary-600 dark:ring-offset-gray-800">
                                </div>
                                <div class="ml-3 text-sm">
                                    <label for="remember" class="text-gray-500 dark:text-gray-300">Remember me</label>
                                </div>
                            </div>
                            <a href="#"
                                class="text-sm font-medium text-primary-600 hover:underline dark:text-primary-500">Forgot
                                password?</a>
                        </div>
                        <button type="submit"
                            class="w-full text-white bg-primary-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800">Sign
                            in</button>
                        <p class="text-sm font-light text-gray-500 dark:text-gray-400">
                            Don’t have an account yet? <a href="#"
                                class="font-medium text-primary-600 hover:underline dark:text-primary-500">Sign up</a>
                        </p>
                    </form>
                    <div class="text-center text-gray-700">Or</div>
                    <p>Login Using the Implicit Flow with Form Post</p>
                    <FwbButtonGroup class="flex flex-col w-full space-y-1">
                        <FwbButton disabled outline @click="loginWithGitHub">Sign In with GitHub</FwbButton>
                        <FwbButton disabled outline @click="loginWithGoogle">Sign In with Google</FwbButton>
                        <FwbButton outline @click="loginWithAuth0">Sign In with Auth0</FwbButton>
                    </FwbButtonGroup>
                </div>
            </div>
        </div>
    </section>
</template>
<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { FwbButton, FwbButtonGroup } from 'flowbite-vue';
import { useRouter } from 'vue-router';
const { loginWithRedirect } = useAuth0();

const router = useRouter();

const loginWithAuth0 = () => {
    loginWithRedirect();
}

const loginWithGitHub = async () => {
    const { authorizeUrl, clientId } = await getAuthFlowInfo('GitHub');
    const state = generateState();
    const url = buildUrl('GitHub', authorizeUrl, clientId, state);
    localStorage.setItem('state', state);
    window.location.href = url;
}

const getAuthFlowInfo = async (provider: string) => {
    try {
        const res = await fetch(`https://localhost:5024/auth/AuthFlowInfo?provider=${provider}`);
        const data = await res.json();
        console.log(data);
        return data;
    } catch (error) {

    }
}

const buildUrl = (provider: string, authorizeUrl: string, clientId: string, state: string) => {
    switch (provider) {
        case 'GitHub':
            //The PKCE (Proof Key for Code Exchange) parameters code_challenge and code_challenge_method are not supported.
            const callback = window.location.protocol + '//' + window.location.host + router.resolve({ name: 'GitHubCallback' }).path;
            const redirect_uri = encodeURIComponent(callback);
            const scope = encodeURIComponent('user');
            return `${authorizeUrl}?client_id=${clientId}&redirect_uri=${redirect_uri}&scope=${scope}&state=${state}`
        default:
            throw new Error('Not supported');
    }
}

const generateState = () => {
    const possible =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    let state = '';
    for (let i = 0; i < 40; i++) {
        state += possible.charAt(Math.floor(Math.random() * possible.length));
    }
    return state;
}

const loginWithGoogle = async () => {

}
</script>