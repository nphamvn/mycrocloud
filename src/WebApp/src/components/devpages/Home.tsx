import routes from "./routes";
import { Link, Outlet, useMatch } from "react-router-dom";

export default function Home() {
  const isHomePage = useMatch("_dev");
  return (
    <div>
      <Link to=".">Home</Link>
      {isHomePage && (
        <ul>
          {routes.map((r) => (
            <li key={r.path}>
              <Link to={r.path}>{r.name}</Link>
            </li>
          ))}
        </ul>
      )}
      <Outlet />
    </div>
  );
}
