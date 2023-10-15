import { useNavigate } from "react-router-dom";
import { User } from "../components/auth/User"
import { GuardedRoute } from "./GuardedRoutes";

export default function RouteAuthGuard(props: GuardedRoute) {
    const navigate = useNavigate();
    const authUser: User = { UserId: 'nam', Role: 'Admin' };
    let allowRoute = false;
    if (authUser) {
        allowRoute = props.allowroles ? props.allowroles.includes(authUser.Role) : true;
    }
    if (allowRoute) {
        return props.component;
    } else {
        navigate(props.redirect ?? '/login', { replace: false })
    }
}