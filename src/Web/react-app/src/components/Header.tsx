import { Link } from "react-router-dom"
import { useAuth } from "../hooks/useAuth";

function Header() {
    const { user } = useAuth();
    return (
        <>
            <h1>MycroCloud</h1>
            <div>Hi, {user?.id}</div>
        </>
    )
}
export default Header