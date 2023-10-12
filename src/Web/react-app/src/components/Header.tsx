import { Link } from "react-router-dom"

function Header() {
    return (
        <nav>
            <ul>
                <li><Link to='/'>Home</Link></li>
            </ul>
        </nav>
    )
}
export default Header