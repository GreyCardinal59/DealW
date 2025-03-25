export default function Home() {

    const styles = {
    container: {
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      height: "80vh",
      backgroundColor: "#f0f0f0"
    },

    title: {
      fontSize: "3rem",
      fontWeight: "bold",
      color: "#333",
    }
  }

  return (<div style={styles.container}>
    <h1 style={styles.title}>DealW</h1>
  </div>)
}
