import { Link } from "react-router-dom";

export default function List() {
  return (
    <div className="mx-auto mt-2 max-w-4xl p-2">
      <div className="flex items-center">
        <h1 className="font-semibold">Your Databases</h1>
        <Link to={"new"} className="ms-auto bg-primary px-2 py-1 text-white">
          New
        </Link>
      </div>
    </div>
  );
}
