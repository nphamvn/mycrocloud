import { useAuth0 } from "@auth0/auth0-react";
import { Avatar, Button, Dropdown, Navbar } from "flowbite-react";
import { Link } from "react-router-dom";
import { useEffect } from "react";

function Header() {
  const {
    isLoading,
    error,
    isAuthenticated,
    user,
    loginWithPopup,
    logout,
    getAccessTokenSilently,
  } = useAuth0();

  useEffect(() => {
    const getUserInfo = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch("/api/me", {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const userId = await res.text();
      console.log(userId);
    };
    if (!isLoading && isAuthenticated) {
      getUserInfo();
    }
  }, [isLoading]);
  if (isLoading) {
    return <div>Loading...</div>;
  }
  if (error) {
    return <div>Oops... {error.message}</div>;
  }

  return (
    <Navbar fluid rounded className="border-b-[1px]">
      <Link to="/" className="">
        <span className="self-center whitespace-nowrap text-xl font-semibold dark:text-white">
          MycroCloud
        </span>
      </Link>
      {isAuthenticated ? (
        <>
          <div className="flex md:order-2">
            <Dropdown
              arrowIcon={false}
              inline
              label={<Avatar alt="User settings" img={user?.picture} rounded />}
            >
              <Dropdown.Header>
                <span className="block">{user?.name}</span>
                <span className="block truncate font-medium">
                  {user?.email}
                </span>
              </Dropdown.Header>
              <Dropdown.Item disabled>Settings</Dropdown.Item>
              <Dropdown.Divider />
              <Dropdown.Item onClick={() => logout()}>Log out</Dropdown.Item>
            </Dropdown>
            <Navbar.Toggle />
          </div>
          <Navbar.Collapse className="items-start">
            <li>
              <Link
                className="block border-b border-gray-100 py-2 pl-3 pr-4 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white md:border-0 md:p-0 md:hover:bg-transparent md:hover:text-cyan-700 md:dark:hover:bg-transparent md:dark:hover:text-white"
                to="apps"
              >
                Apps
              </Link>
            </li>
          </Navbar.Collapse>
        </>
      ) : (
        <Button size="sm" onClick={() => loginWithPopup()}>
          Log in
        </Button>
      )}
    </Navbar>
  );
}
export default Header;
