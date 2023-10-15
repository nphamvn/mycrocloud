import { useEffect, useState } from "react";
import Route from "./Route";
import routes from '../../data/routes.json';
import { useForm } from "react-hook-form";

type Inputs = {
    name: string,
    path: string,
    method: string
};

export default function RouteCreateUpdate({ routeId }: { routeId: number | undefined }) {
    const isEditMode = routeId !== undefined;
    const [route, setRoute] = useState<Route>();

    useEffect(() => {
        if (isEditMode) {
            setRoute(routes.find(r => r.id === routeId)!);
        }
    }, [routeId]);

    const { register, handleSubmit, formState: { errors }, } = useForm<Inputs>();
    const onSubmit = (data) => {
        alert(JSON.stringify(data))
    }
    const isSupportedMethods = (methods: string) =>{
        alert(methods);
        return true;
    }
    return (
        <>
            <h1>{!isEditMode ? 'Create New Route' : 'Edit Route'}</h1>
            <form onSubmit={handleSubmit(onSubmit)}>
                <div>
                    <label htmlFor="name">Name</label>
                    <input type="text" id="name" {...register('name')} />
                    {errors.name && <span>{errors.name.message}</span>}
                </div>
                <div>
                    <label htmlFor="method">Method</label>
                    <input type="text" id="method" {...register('method', { validate: isSupportedMethods })} />
                    {errors.method && <span>{errors.method.message}</span>}
                </div>
                <div>
                    <label htmlFor="path">Path</label>
                    <input type="text" id="path" {...register('path')} />
                </div>
                <button type="submit">Save</button>
            </form>
        </>
    )
}