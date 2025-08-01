import type React from "react";
import Header from "./Header";

const PageLayout = ({ children }: { children: React.ReactNode})  => {
    return (
      <>
        <Header />
        {children}
      </>
    )
}

export default PageLayout;