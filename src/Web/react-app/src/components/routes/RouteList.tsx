import { useParams } from "react-router-dom"
import RouteCreateUpdate from "./RouteCreateUpdate";
import { useEffect, useState } from "react";
import Route from "./Route";
import routeData from "../../data/routes.json";

function RouteList() {
    const { appId } = useParams();
    const [routes, setRoutes] = useState<Route[]>([]);
    const [routeId, setRouteId] = useState<number | undefined>();

    useEffect(() => {
        setRoutes(routeData);
    }, []);

    return (
        <>
            <h1>Routes</h1>
            <button onClick={() => setRouteId(undefined)}>New</button>
            <ul>
                {routes.map(route => {
                    return (<li key={route.id} onClick={() => setRouteId(route.id)}>{route.name}</li>)
                })}
            </ul>
            <RouteCreateUpdate routeId={routeId} />
        </>
    )
}
export default RouteList