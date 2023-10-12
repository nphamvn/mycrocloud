import { useParams } from "react-router-dom"

function RouteCreateUpdate() {
    const { routeId } = useParams();
    return (
        <h1>{!routeId ? "Create New Route" : "Edit Route"} </h1>
    )
}
export default RouteCreateUpdate