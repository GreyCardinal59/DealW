import Layout, { Header, Content, Footer } from "antd/es/layout/layout";
import "./globals.css";
import { Menu } from "antd";
import Link from "next/link";

const items = [
  {key: "home", label: <Link href={"/"}>Главная</Link>},
  {key: "theory", label: <Link href={"/theory"}>Теория</Link>},
  {key: "quizzes", label: <Link href={"/quizzes"}>Квизы</Link>},
];

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <Layout style={{minHeight: "100vh"}}>
          <Header>
            <Menu 
                theme ="dark" 
                mode="horizontal" 
                items = {items} 
                style= {{flex: 1, minWidth: 0, justifyContent: "center"}}
            />
          </Header>
          <Content style={{padding: "0 48px"}}>{children}</Content>
          <Footer style={{textAlign: "center"}}>
            DealW 2024 by LAMENT & Grey Cardinal
          </Footer>
        </Layout>
      </body>
    </html>
  );
};
