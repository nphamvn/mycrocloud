import { Link } from "react-router-dom"
import MockAppList from "./App"

function AppList() {
    return (
        <>
            <Link to='/apps/new'>Create New App</Link>
            <ul>
                {
                    MockAppList.map(app => {
                        return <li key={app.AppId}>{app.AppName}</li>
                    })
                }
            </ul>
        </>
    )
}
export default AppList