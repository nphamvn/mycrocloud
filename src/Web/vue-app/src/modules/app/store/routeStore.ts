import { defineStore } from "pinia";
import { ROUTE_METHODS } from "../constants";
import { ref } from "vue";
import RouteItem, { MockResponse } from "../models/RouteItem";

export const useRouteStore = defineStore('route', () => {
    const routes = ref<RouteItem[]>([]);
    const openingRoute = ref<RouteItem>();
    const getRoutes = async () => {
        routes.value = Array.from({ length: 10 }, (_, index) => ({
            id: index + 1,
            name: `Route ${index + 1}`,
            method: ROUTE_METHODS[Math.floor(Math.random() * ROUTE_METHODS.length)],
            path: `/foo/${index + 1}`,
            desciption: `Route ${index + 1}`,
            responseType: 'mock'
        }));
    }

    const setOpeningRoute = (route : RouteItem) => {
        openingRoute.value = route;
    }

    const createRoute = async (route: RouteItem) => {
        const newRoute = {...route, id: routes.value.length + 1};
        routes.value = [...routes.value, newRoute];
        return newRoute;
    }

    const getRouteById = async (id: number) => {
        return routes.value.find(r => r.id === id);
    }

    const updateRoute = async (route: RouteItem) => {
        const id = route.id;
        const index = routes.value.findIndex(r => r.id === id);
        if (index >= 0) {
            routes.value[index] = route;
        }
    }

    const setRouteResponse = async (id: number, response: MockResponse) => {
        const index = routes.value.findIndex(r => r.id === id);
        if (index >= 0) {
            routes.value[index].response = response;
        }
    }

    const deleteRoute = async (id: number) => {
        const index = routes.value.findIndex(r => r.id === id);
        if (index >= 0) {
            routes.value.splice(index, 1);
        }
    }

    return {
        routes,
        getRoutes,
        openingRoute,
        setOpeningRoute,
        createRoute,
        getRouteById,
        updateRoute,
        setRouteResponse,
        deleteRoute
    };
})
