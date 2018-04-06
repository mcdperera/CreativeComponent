
import com.google.gson.Gson;
import java.net.*;
import java.io.*;

/*
 * The Client that can be run both as a console or a GUI
 */
//<remarks>http://www.dreamincode.net/forums/topic/259777-a-simple-chat-program-with-clientserver-gui-optional/</remarks>
public class Client {

//    /**
//     * The Input stream
//     */
//    public ObjectInputStream sInput;
//
//    /**
//     * The output stream
//     */
//    private ObjectOutputStream sOutput;
//    DataOutputStream outputStream;
//    DataInputStream inputStream;

    
    OutputStream outputStream;
    InputStream inputStream;

    
    /**
     * The socket
     */
    private Socket socket;

    /**
     * Construct the client object.
     */
    Client() {

    }

    /**
     * Returns the match stat message.
     */
    public boolean start(String server, String port, String username) {

        try {
            socket = new Socket(server, Integer.parseInt(port));
        } catch (Exception ec) {
            display("Error connectiong to server:" + ec);
            return false;
        }

        try {
//            inputStream = new DataInputStream(socket.getInputStream());
//
//            outputStream = new DataOutputStream(socket.getOutputStream());

            outputStream = socket.getOutputStream();
            inputStream = socket.getInputStream();

        } catch (IOException eIO) {
            display("Exception creating new Input/output Streams: " + eIO);
            return false;
        }

        Message message = new Message(MessageType.CONNECTIONESTABLISH_CLIENTREQUEST.getValue(),
                true, "username send", false, ErrorMessageType.NONE.getValue());

        message.ConnectionMessage = (new ConnectionMessage("", username));

        this.sendMessage(message);

        return true;
    }

    /*
	 * To send a message to the console or the GUI
     */
    public static void display(String msg) {
        System.out.println(msg);

    }

    /*
	 * To send a message to the server
     */
    void sendMessage(Message msg) {

        try {
            String json = new Gson().toJson(msg);

            byte[] bytes = json.getBytes();
            outputStream.write(bytes, 0, bytes.length);
            outputStream.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
//        try {
//
//            // Sending
//            String toSend = new Gson().toJson(msg);
//
//            byte[] toSendBytes = toSend.getBytes();
//            int toSendLen = toSendBytes.length;
//            byte[] toSendLenBytes = new byte[4];
//            toSendLenBytes[0] = (byte) (toSendLen & 0xff);
//            toSendLenBytes[1] = (byte) ((toSendLen >> 8) & 0xff);
//            toSendLenBytes[2] = (byte) ((toSendLen >> 16) & 0xff);
//            toSendLenBytes[3] = (byte) ((toSendLen >> 24) & 0xff);
//            outputStream.write(toSendLenBytes);
//            outputStream.write(toSendBytes);
//            display("write output");
//            //outputStream.w.writeObject(msg);
//        } catch (IOException e) {
//            display("Exception writing to server: " + e);
//        }
    }

    
      public  byte[] intToByteArray(int a) {
            byte[] ret = new byte[4];
            ret[0] = (byte) (a & 0xFF);
            ret[1] = (byte) ((a >> 8) & 0xFF);
            ret[2] = (byte) ((a >> 16) & 0xFF);
            ret[3] = (byte) ((a >> 24) & 0xFF);
            return ret;
        }
      
    /*
	 * When something goes wrong
	 * Close the Input/Output streams and disconnect not much to do in the catch clause
     */
    public void disconnect() {
        try {
            if (inputStream != null) {
                inputStream.close();
            }
        } catch (Exception e) {
        } // not much else I can do
        try {
            if (outputStream != null) {
                outputStream.close();
            }
        } catch (Exception e) {
        } // not much else I can do
        try {
            if (socket != null) {
                socket.close();
            }
        } catch (Exception e) {
        } // not much else I can do

    }

}
