import Home from "../components/Home";

export type GuardedRoute = {
    path: string;
    component: React.FC,
    redirect?: string,
    allowroles?: string[]
}
const GuardedRoutes: GuardedRoute[] = [
    { path: '', component: Home }
]

export default GuardedRoutes;