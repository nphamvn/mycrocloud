import { Link } from "react-router-dom"
import MockAppList from "./App"
import { useAuth } from "../../hooks/useAuth"

function AppList() {
    const { user } = useAuth();
    
    return (
        <>
            <Link to='/apps/new'>Create New App</Link>
            <ul>
                {
                    MockAppList.map(app => {
                        return <li key={app.AppId}><Link to={`/apps/${app.AppId}/routes`}>{app.AppName}</Link></li>
                    })
                }
            </ul>
        </>
    )
}
export default AppList