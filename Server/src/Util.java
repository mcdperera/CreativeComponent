/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author charmdm
 */
public class Util {
      public  static byte[] intToByteArray(int a) {
            byte[] ret = new byte[4];
            ret[0] = (byte) (a & 0xFF);
            ret[1] = (byte) ((a >> 8) & 0xFF);
            ret[2] = (byte) ((a >> 16) & 0xFF);
            ret[3] = (byte) ((a >> 24) & 0xFF);
            return ret;
        }
}
