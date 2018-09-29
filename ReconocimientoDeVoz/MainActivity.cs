using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Java.Util;
using Android.Content.PM;
using Android.Provider;

namespace ReconocimientoDeVoz
{
    [Activity(Label = "ReconocimientoDeVoz", MainLauncher = true, Icon = "@drawable/icon")]
    [IntentFilter(new[] { MediaStore.ActionImageCapture }, Categories = new[] { Intent.CategoryDefault })]
    [IntentFilter(new[] { MediaStore.ActionImageCaptureSecure }, Categories = new[] { Intent.CategoryDefault })]
    [IntentFilter(new[] { MediaStore.ActionImageCapture }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
    [IntentFilter(new[] { MediaStore.ActionImageCaptureSecure }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryVoice })]
    public class MainActivity : Activity
    {
        int VOICE = 10;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;

            if (rec != "android.hardware.microphone")
            {
                var alert = new AlertDialog.Builder(button.Context);
                alert.SetTitle("su dispositivo no posee microfono");
                alert.SetPositiveButton("OK", (sender, e) => { return; });
                alert.Show();

            }

            button.Click += StartVoiceRecognitionActivity;
        }

        public void StartVoiceRecognitionActivity(object s, EventArgs e)
        {
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Locale.Default);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.SpeakNow));
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            try
            {
                StartActivityForResult(voiceIntent, VOICE);
            }
            catch (ActivityNotFoundException ex)
            {
                Toast.MakeText(ApplicationContext, "Tu dispositivo no soporta el reconocimiento de voz", ToastLength.Long).Show();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultCode == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string[] words = matches[0].ToString().Split(' ');
                        if (words[0].Equals("Abrir"))
                        {
                            var AppName = words[1];
                            OpenApp(ApplicationContext, AppName);
                        }
                    }
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void OpenApp(Context applicationContext, string appName)
        {

            var packagename = getApplicationPackageName(applicationContext, appName);

            Intent i;

            PackageManager manager = PackageManager;

            try
            {
                i = manager.GetLaunchIntentForPackage(packagename);
                if (i == null)
                    throw new PackageManager.NameNotFoundException();
                i.AddCategory(Intent.CategoryLauncher);
                StartActivity(i);
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Toast.MakeText(ApplicationContext, "no se encontro dicho nombre", ToastLength.Long).Show();
            }

        }

        private string getApplicationPackageName(Context applicationContext, string appName)
        {
            var applications = PackageManager.GetInstalledApplications(PackageInfoFlags.MetaData);



            foreach (var app in applications)
            {
                var name = app.LoadLabel(PackageManager);
                if (appName.ToLower() == name.ToLower())
                    return app.PackageName;

            }

            return PackageName;


        }


    }
}

