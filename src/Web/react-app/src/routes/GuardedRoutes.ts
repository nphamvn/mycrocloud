import Home from "../components/Home";

export type GuardedRoute = {
    path: string;
    component: React.ReactNode,
    redirect?: string,
    allowroles?: string[]
}
// <Route path='/' Component={Home}></Route>
//             <Route path='/apps' element={<AppList />} />
//             <Route path='/apps/new' element={<AppCreate />} />
//             <Route path='/apps/:appId/routes' element={<RouteList />} />
//             <Route path='/apps/:appId/routes/new' element={<RouteCreateUpdate />} />
//             <Route path='/apps/:appId/routes/:routeId' element={<RouteCreateUpdate />} />
const GuardedRoutes: GuardedRoute[] = [
    { path: '', component: Home }
]

export default GuardedRoutes;