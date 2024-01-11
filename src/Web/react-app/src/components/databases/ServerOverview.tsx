import { Link, useParams } from "react-router-dom";

export default function ServerOverview() {
  const id = parseInt(useParams()["id"]!);
  return (
    <div>
      <h1>Server Overview</h1>
      <Link to={`/dbservers/${id}/dbs`}>Databases</Link>
    </div>
  );
}
