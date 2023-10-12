import { Link, useParams } from "react-router-dom"
import { App } from "../apps/App"
function RouteList() {
    const { appId } = useParams();
    const app: App = { AppId: parseInt(appId!), AppName: 'My App' };
    return (
        <>
            <h1>Routes</h1>
            <Link to={`/apps/${app.AppId}/routes/new`}>New Route</Link>
            <ul>
                <li>Route 1</li>
                <li>Route 2</li>
                <li>Route 3</li>
            </ul>
        </>
    )
}
export default RouteList