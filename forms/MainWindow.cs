namespace tempest;

public partial class MainWindow : Form {
  public MainWindow() {
    InitializeComponent();
    var replay = new Replay();
    replay.getPosition();
    replay.setPosition(10000);
  }
}