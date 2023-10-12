import { Link } from "react-router-dom"

function Home() {
    return (
        <>
            <h1>Welcome to MycroCloud</h1>
            <Link to='/apps'>Apps</Link>
        </>
    )
}
export default Home