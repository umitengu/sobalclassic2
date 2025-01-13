import Foundation
import Photos

/// 写真撮影後のコールバック
public typealias OnCapturedCameraImage = @convention(c) () -> Void
public typealias OnCancelCapturedCameraImage = @convention(c) () -> Void
 
public class MocopiUIViewController: UIViewController, UIImagePickerControllerDelegate, UINavigationControllerDelegate {
    
    // シングルトン化
    public static let shared = MocopiUIViewController()

    public static let cameraPicker = UIImagePickerController()
    
    // コールバック登録
    private var capturedCameraImageCallback: OnCapturedCameraImage?
    private var cancelCapturedCameraImageCallback: OnCancelCapturedCameraImage?
    @objc static public func registerOnCapturedCameraImage(callback: @escaping OnCapturedCameraImage) {
        MocopiUIViewController.shared.capturedCameraImageCallback = callback
    }
    @objc static public func registerOnCancelCapturedCameraImage(callback: @escaping OnCancelCapturedCameraImage) {
        MocopiUIViewController.shared.cancelCapturedCameraImageCallback = callback
    }
    
    /// 正しい向きに回転したUIImageを取得
    ///
    /// - Parameters:
    ///   - uiImage: 向きを確認して回転させるUIImage
    /// - Returns: 回転後のUIImage
    private func getCorrectOrientationUIImage(uiImage:UIImage) -> UIImage {
        var newImage = UIImage()
        let ciContext = CIContext(options: nil)
        switch uiImage.imageOrientation {
        case .down:
            // 180°回転で正しい向きになる状態
            guard let orientedCIImage = CIImage(image: uiImage)?.oriented(CGImagePropertyOrientation.down),
                let cgImage = ciContext.createCGImage(orientedCIImage, from: orientedCIImage.extent)
                else {
                    print("Image rotation failed.")
                    return uiImage
                }
                newImage = UIImage(cgImage: cgImage)
        case .right:
            // 時計回り90°回転で正しい向きになる状態
            guard let orientedCIImage = CIImage(image: uiImage)?.oriented(CGImagePropertyOrientation.right),
                let cgImage = ciContext.createCGImage(orientedCIImage, from: orientedCIImage.extent)
                else {
                    print("Image rotation failed.")
                    return uiImage
                }
                newImage = UIImage(cgImage: cgImage)
        default:
            newImage = uiImage
        }
        return newImage
    }
}
